import { Button } from '@material-ui/core'
import DeleteGroupPrompt from 'components/AppNavbar/NavbarActions/DeleteGroupPrompt'
import React, { useState } from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { groupsSlice, blogsSlice, snackbarSlice } from 'store/slices'
import DeleteIcon from '@material-ui/icons/Delete'
import { useSelector } from 'react-redux'
import { Blog } from 'api/api.types'

export interface RemoveGroupBtnProps {
  id: number
  curGroupId: number
}

export const RemoveGroupBtn: React.FC<RemoveGroupBtnProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const blogsList = useSelector(blogsSlice.selectAll)
  const [showPrompt, setShowPrompt] = useState(false)
  const [inAction, setInAction] = useState(false)

  const getBlogs = () => {
    const blogs: Blog[] = []

    blogsList
      .filter(x => x.userData.groupId == props.id)
      .map(el => blogs.push(el))

    return blogs
  }

  const removeGroup = async (id: number, moveSubsToAll: boolean) => {
    if (inAction) return

    setInAction(true)

    const subs_to_change = getBlogs()

    const promise = await dispatch(
      groupsSlice.remove({
        groupId: id,
        unsubscribeSubscriptions: !moveSubsToAll,
      })
    )

    if (groupsSlice.remove.fulfilled.match(promise)) {
      if (moveSubsToAll) {
        //reset all unknown groups
        for (const sub of subs_to_change) {
          dispatch(blogsSlice.actions.resetGroup(sub.id))
        }
      } else {
        //remove all subs with unknown group
        for (const sub of subs_to_change) {
          dispatch(blogsSlice.actions.remove(sub.id))
        }
      }

      if (props.curGroupId == id) push('/')

      dispatch(
        snackbarSlice.actions.setSnackbar({
          open: true,
          color: 'success',
          msg: 'Group removed',
        })
      )
    } else {
      dispatch(
        snackbarSlice.actions.setSnackbar({
          open: true,
          color: 'error',
          msg: 'Failed to remove group',
        })
      )
    }

    setInAction(false)
  }

  const renderDeletePrompt = () => {
    return
  }

  return (
    <React.Fragment>
      <Button
        onClick={() => {
          setShowPrompt(true)
        }}
      >
        <DeleteIcon />
      </Button>
      {renderDeletePrompt()}
    </React.Fragment>
  )
}

export default RemoveGroupBtn
