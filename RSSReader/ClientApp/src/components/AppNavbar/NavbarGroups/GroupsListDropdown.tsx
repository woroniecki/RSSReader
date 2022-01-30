import { Collapse, List, ListItem } from '@material-ui/core'
import { ExpandLess, ExpandMore } from '@material-ui/icons'
import BlogAvatar from 'components/Blog/BlogAvatar'
import React from 'react'
import { useSelector } from 'react-redux'
import { useHistory, useParams } from 'react-router-dom'
import { blogsSlice } from 'store/slices'
import AppListItemText from '../AppListItemText'
import { useStyles } from '../UserNavbar'
import { getUrlWithGroupId } from 'utils/utils'

export interface GroupsListDropdownProps {
  groupId?: number
  pushTo: string
  groupName: string
  dropdown: boolean
  allBlogs?: boolean
}

export const GroupsListDropdown: React.FC<GroupsListDropdownProps> = props => {
  const { push } = useHistory()
  const blogsList = useSelector(blogsSlice.selectAll)
  const { groupId } = useParams<{ groupId: string }>()
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
          onClick={() => push(getUrlWithGroupId('/blog/' + el.id.toString(), groupId))}
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

  const renderDropdownIcon = () => {
    if (!props.dropdown) return
    return open ? <ExpandLess /> : <ExpandMore />
  }

  return (
    <List>
      <ListItem
        selected={
          props.groupId == parseInt(groupId) ||
          (isNaN(parseInt(groupId)) && isNaN(props.groupId))
        }
        key={'grouplist' + props.groupId}
        button
        onClick={handleClick}
      >
        <AppListItemText fontSize={14} variant="h3" text={props.groupName} />
        {renderDropdownIcon()}
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
