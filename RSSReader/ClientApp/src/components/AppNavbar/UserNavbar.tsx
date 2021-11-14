import { Collapse, List, ListItem } from '@material-ui/core'
import Divider from '@material-ui/core/Divider'
import { makeStyles } from '@material-ui/core/styles'
import CreateIcon from '@material-ui/icons/Create'
import ExitToAppIcon from '@material-ui/icons/ExitToApp'
import ExpandLess from '@material-ui/icons/ExpandLess'
import ExpandMore from '@material-ui/icons/ExpandMore'
import PersonIcon from '@material-ui/icons/Person'
import PersonAddIcon from '@material-ui/icons/PersonAdd'
import React from 'react'
import { useSelector } from 'react-redux'
import { useHistory } from 'react-router-dom'
import { authSlice, blogsSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import AppListItemIcon from './AppListItemIcon'
import AppListItemText from './AppListItemText'
import GroupsNavlist from './NavbarGroups/GroupsNavlist'

export interface UserNavbarProps {}

export const UserNavbar: React.FC<UserNavbarProps> = () => {
  const { push } = useHistory()
  const { userName } = useSelector(authSlice.stateSelector)
  const dispatch = useAppDispatch()

  const classes = useStyles()
  const [userPanelDropdown, setUserPanelDropdown] = React.useState(false)

  const handleClick = () => {
    setUserPanelDropdown(!userPanelDropdown)
  }

  let returnValue

  function OnLogout() {
    dispatch(authSlice.actions.logout())
    dispatch(blogsSlice.actions.clear())
  }

  if (!userName) {
    returnValue = (
      <List>
        <ListItem button key="login" onClick={() => push(`/login`)}>
          <AppListItemIcon>
            <CreateIcon />
          </AppListItemIcon>
          <AppListItemText fontSize={16} variant="h2" text="Login" />
        </ListItem>
        <ListItem button key="Register" onClick={() => push(`/register`)}>
          <AppListItemIcon>
            <PersonAddIcon />
          </AppListItemIcon>
          <AppListItemText fontSize={16} variant="h2" text="Register" />
        </ListItem>
      </List>
    )
  } else {
    returnValue = (
      <React.Fragment>
        <List>
          <ListItem key="Userpanel" button onClick={handleClick}>
            <AppListItemIcon>
              <PersonIcon fontSize="small" />
            </AppListItemIcon>
            <AppListItemText fontSize={16} variant="h2" text={userName} />
            {userPanelDropdown ? <ExpandLess /> : <ExpandMore />}
          </ListItem>
          <Collapse in={userPanelDropdown} timeout="auto" unmountOnExit>
            <List component="div" disablePadding>
              <ListItem button onClick={OnLogout}>
                <AppListItemIcon style={{ margin: '0px 4px 0px 16px' }}>
                  <ExitToAppIcon fontSize="small" />
                </AppListItemIcon>
                <AppListItemText fontSize={12} variant="button" text="Logout" />
              </ListItem>
            </List>
          </Collapse>
        </List>
        <Divider />
        <GroupsNavlist />
      </React.Fragment>
    )
  }

  return returnValue
}

export const useStyles = makeStyles(theme => ({
  root: {
    width: '100%',
    maxWidth: 360,
    backgroundColor: theme.palette.background.paper,
  },
  nested: {
    paddingLeft: theme.spacing(4),
  },
}))

export default UserNavbar
