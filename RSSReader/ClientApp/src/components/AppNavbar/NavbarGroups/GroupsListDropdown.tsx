import {
  Collapse,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
} from '@material-ui/core'
import { ExpandLess, ExpandMore } from '@material-ui/icons'
import React from 'react'
import { useHistory } from 'react-router-dom'
import { useStyles } from '../UserNavbar'
import { blogsSlice } from 'store/slices'
import { useSelector } from 'react-redux'
import RemoveGroupBtn from './RemoveGroupBtn'

export interface GroupsListDropdownProps {
  groupId: number
  pushTo: string
  groupName: string
  dropdown: boolean
  allBlogs?: boolean
}

export const GroupsListDropdown: React.FC<GroupsListDropdownProps> = props => {
  const { push } = useHistory()
  const blogsList = useSelector(blogsSlice.selectAll)

  const classes = useStyles()
  const [open, setOpen] = React.useState(false)

  const handleClick = () => {
    setOpen(!open)
    push(props.pushTo)
  }

  const renderBlogs = () => {
    return blogsList
      .filter(
        el =>
          el.userData.groupId == props.groupId ||
          (props.groupId == -1 && el.userData.groupId == null) ||
          props.allBlogs
      )
      .map(el => (
        <ListItem
          button
          className={classes.nested}
          key={el.id}
          onClick={() => push(`/blog/` + el.id.toString())}
        >
          <ListItemIcon></ListItemIcon>
          <ListItemText primary={el.name} />
        </ListItem>
      ))
    //
  }

  const renderDeleteBtn = () => {
    if (props.groupId >= 0)
      return <RemoveGroupBtn id={props.groupId} curGroupId={props.groupId} />
  }

  const renderDropdownIcon = () => {
    if (!props.dropdown) return
    return open ? <ExpandLess /> : <ExpandMore />
  }

  return (
    <List>
      <ListItem key="Userpanel" button onClick={handleClick}>
        <ListItemIcon></ListItemIcon>
        <ListItemText primary={props.groupName} />
        {renderDropdownIcon()}
        {renderDeleteBtn()}
      </ListItem>
      <Collapse in={open && props.dropdown} timeout="auto" unmountOnExit>
        <List component="div" disablePadding>
          {renderBlogs()}
        </List>
      </Collapse>
    </List>
  )
}

export default GroupsListDropdown
