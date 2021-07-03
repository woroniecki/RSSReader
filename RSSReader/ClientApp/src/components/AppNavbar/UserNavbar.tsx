import React from 'react'
import CreateIcon from '@material-ui/icons/Create'
import PersonAddIcon from '@material-ui/icons/PersonAdd'
import PersonIcon from '@material-ui/icons/Person'
import ExitToAppIcon from '@material-ui/icons/ExitToApp'
import { useHistory } from 'react-router-dom'
import { makeStyles } from '@material-ui/core/styles'
import ExpandLess from '@material-ui/icons/ExpandLess'
import ExpandMore from '@material-ui/icons/ExpandMore'
import Divider from '@material-ui/core/Divider'
import { useSelector } from 'react-redux'
import { authSlice } from 'store/slices'
import { blogsSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import GroupsNavlist from './NavbarGroups/GroupsNavlist'
import {
  Collapse,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
} from '@material-ui/core'

export interface UserNavbarProps {
  curGroupId: number
}

export const UserNavbar: React.FC<UserNavbarProps> = props => {
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
          <ListItemIcon>
            <CreateIcon />
          </ListItemIcon>
          <ListItemText primary="Login" />
        </ListItem>
        <ListItem button key="Register" onClick={() => push(`/register`)}>
          <ListItemIcon>
            <PersonAddIcon />
          </ListItemIcon>
          <ListItemText primary="Register" />
        </ListItem>
      </List>
    )
  } else {
    returnValue = (
      <React.Fragment>
        <List>
          <ListItem key="Userpanel" button onClick={handleClick}>
            <ListItemIcon>
              <PersonIcon />
            </ListItemIcon>
            <ListItemText primary={userName} />
            {userPanelDropdown ? <ExpandLess /> : <ExpandMore />}
          </ListItem>
          <Collapse in={userPanelDropdown} timeout="auto" unmountOnExit>
            <List component="div" disablePadding>
              <ListItem button className={classes.nested} onClick={OnLogout}>
                <ListItemIcon>
                  <ExitToAppIcon />
                </ListItemIcon>
                <ListItemText primary="Logout" />
              </ListItem>
            </List>
          </Collapse>
        </List>
        <Divider />
        <GroupsNavlist curGroupId={props.curGroupId} />
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
