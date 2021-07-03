import {
  Button,
  Collapse,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
} from '@material-ui/core'
import { ExpandLess, ExpandMore } from '@material-ui/icons'
import React from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { useStyles } from '../UserNavbar'
import { blogsSlice } from 'store/slices'
import { useSelector } from 'react-redux'
import RemoveGroupBtn from './RemoveGroupBtn'

export interface GroupsListDropdownProps {
  groupId: number
  pushTo: string
  groupName: string
  allBlogs?: boolean
}

export const GroupsListDropdown: React.FC<GroupsListDropdownProps> = props => {
  const dispatch = useAppDispatch()
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

  return (
    <List>
      <ListItem key="Userpanel" button onClick={handleClick}>
        <ListItemIcon></ListItemIcon>
        <ListItemText primary={props.groupName} />
        {open ? <ExpandLess /> : <ExpandMore />}
        {renderDeleteBtn()}
      </ListItem>
      <Collapse in={open} timeout="auto" unmountOnExit>
        <List component="div" disablePadding>
          {renderBlogs()}
        </List>
      </Collapse>
    </List>
  )
}

export default GroupsListDropdown
