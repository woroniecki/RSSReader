import React, { useState } from 'react'
import { useHistory } from 'react-router-dom'
import { useSelector } from 'react-redux'

import { authSlice, groupsSlice, blogsSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import AddGroupBtn from './AddGroupBtn'
import GroupsListDropdown from './GroupsListDropdown'

export interface GroupsNavlistProps {
  curGroupId: number
}

export const GroupsNavlist: React.FC<GroupsNavlistProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { token } = useSelector(authSlice.stateSelector)
  const groupsList = useSelector(groupsSlice.selectAll)
  const blogsList = useSelector(blogsSlice.selectAll)

  const fetchList = async () => {
    const promise = await dispatch(groupsSlice.getList())

    if (groupsSlice.getList.fulfilled.match(promise)) {
    } else {
    }
  }

  React.useEffect(() => {
    if (token) {
      fetchList()
    }
  }, [token])

  const renderAllGroup = () => {
    return (
      <GroupsListDropdown
        groupId={-1}
        groupName={'All'}
        pushTo={`/`}
        allBlogs={true}
      />
    )
  }

  const renderNoneGroup = () => {
    if (
      blogsList.filter(
        x => x.userData.groupId == -1 || x.userData.groupId == null
      ).length > 0
    ) {
      return (
        <GroupsListDropdown groupId={-1} groupName={'None'} pushTo={`/-1`} />
      )
    }
  }

  const renderGroupsList = () =>
    groupsList
      .filter(el => el.id != -1)
      .map(el => (
        <GroupsListDropdown
          key={el.id}
          groupId={el.id}
          groupName={el.name}
          pushTo={`/` + el.id.toString()}
        />
      ))

  return (
    <>
      {renderAllGroup()}
      {renderNoneGroup()}
      {renderGroupsList()}
      <AddGroupBtn />
    </>
  )
}

export default GroupsNavlist
