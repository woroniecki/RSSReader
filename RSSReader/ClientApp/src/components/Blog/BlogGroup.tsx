import React from 'react'
import { Dropdown, DropdownButton } from 'react-bootstrap'
import { useHistory, useParams } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { groupsSlice, subscriptionsSlice } from 'store/slices'
import { useSelector } from 'react-redux'

export interface BlogGroupProps {
  subId: number
  activeGroupId: number
}

export const BlogGroup: React.FC<BlogGroupProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const groupsList = useSelector(groupsSlice.selectAll)
  const none_group = 'None'

  const getActiveGroup = () => {
    const group = groupsList.find(el => el.id == props.activeGroupId)
    return group != null ? group.name : none_group
  }

  const assignGroup = async (groupId: number) => {
    if (groupId == props.activeGroupId) return

    const promise = await dispatch(
      subscriptionsSlice.patchGroup({
        subId: props.subId,
        groupId: groupId,
      })
    )

    if (groupsSlice.remove.fulfilled.match(promise)) {
    } else {
    }
  }

  const renderGroupsList = () => {
    return groupsList.map(el => (
      <Dropdown.Item
        key={el.id}
        active={el.id == props.activeGroupId}
        onClick={() => assignGroup(el.id)}
      >
        {el.name}
      </Dropdown.Item>
    ))
  }

  return (
    <Dropdown>
      <Dropdown.Toggle variant="success" id="dropdown-basic">
        {getActiveGroup()}
      </Dropdown.Toggle>
      <Dropdown.Menu>{renderGroupsList()}</Dropdown.Menu>
    </Dropdown>
  )
}

export default BlogGroup
