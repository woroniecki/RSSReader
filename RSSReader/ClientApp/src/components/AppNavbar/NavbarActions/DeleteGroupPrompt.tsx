import React, { useState } from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { groupsSlice, blogsSlice, snackbarSlice } from 'store/slices'
import DeleteIcon from '@material-ui/icons/Delete'
import { useSelector } from 'react-redux'
import { Blog } from 'api/api.types'
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from '@material-ui/core'
import SpinnerElement from 'components/Spinner/SpinnerElement'

export interface DeleteGroupPromptProps {
  onClose: (event: React.MouseEvent<HTMLButtonElement>) => void
  groupId: number
}

export const DeleteGroupPrompt: React.FC<DeleteGroupPromptProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const blogsList = useSelector(blogsSlice.selectAll)
  const groupsList = useSelector(groupsSlice.selectAll)
  const [inAction, setInAction] = useState(false)

  const getGroupName = () => {
    const groups = groupsList.filter(x => x.id == props.groupId)

    if (groups.length > 0) return groups[0].name
  }

  const getBlogs = () => {
    const blogs: Blog[] = []

    blogsList
      .filter(x => x.userData.groupId == props.groupId)
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

      if (props.groupId == id) push('/')

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
    props.onClose(this)
  }

  function renderProcessing() {
    if (!inAction) return
    return <SpinnerElement size={24} />
  }

  return (
    <>
      <Dialog
        open={true}
        onClose={props.onClose}
        aria-labelledby="form-dialog-title"
      >
        <DialogTitle id="form-dialog-title">Delete group</DialogTitle>
        <DialogContent>
          <DialogContentText>
            What to do with blogs assigned to group {getGroupName()}?
          </DialogContentText>
          <div
            style={{
              display: 'flex',
              justifyContent: 'center',
              alignItems: 'center',
            }}
          >
            {renderProcessing()}
          </div>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => removeGroup(props.groupId, true)}>
            Move to all
          </Button>
          <Button onClick={() => removeGroup(props.groupId, false)}>
            Delete
          </Button>
          <Button onClick={props.onClose}>Close</Button>
        </DialogActions>
      </Dialog>
    </>
  )
}

export default DeleteGroupPrompt
