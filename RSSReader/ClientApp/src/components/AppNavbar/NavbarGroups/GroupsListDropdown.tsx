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
import BlogAvatar from 'components/Blog/BlogAvatar'
import AppListItemText from '../AppListItemText'

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
          key={el.id}
          onClick={() => push(`/blog/` + el.id.toString())}
        >
          <BlogAvatar
            title={el.name}
            imageUrl={el.imageUrl}
            size="small"
            variant="rounded"
          />
          <AppListItemText fontSize={12} variant="button" text={el.name} />
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
        <AppListItemText fontSize={14} variant="h3" text={props.groupName} />
        {renderDropdownIcon()}
        {renderDeleteBtn()}
      </ListItem>
      <Collapse in={open && props.dropdown} timeout="auto" unmountOnExit>
        <List component="div" disablePadding={false}>
          {renderBlogs()}
        </List>
      </Collapse>
    </List>
  )
}

export default GroupsListDropdown
