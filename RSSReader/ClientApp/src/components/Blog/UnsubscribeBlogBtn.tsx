import { Button } from '@material-ui/core'
import DeleteIcon from '@material-ui/icons/Delete'
import { Blog } from 'api/api.types'
import SpinnerElement from 'components/Spinner/SpinnerElement'
import React, { useState } from 'react'
import { blogsSlice, snackbarSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'

export interface UnsubscribeBlogBtnProps {
  blog: Blog
}

export const UnsubscribeBlogBtn: React.FC<UnsubscribeBlogBtnProps> = props => {
  const dispatch = useAppDispatch()
  const [isInAction, setIsInAction] = useState(false)

  const unsubcribeBlog = async () => {
    if (isInAction) return
    setIsInAction(true)
    const promise = await dispatch(blogsSlice.putUnsubscribeBlog(props.blog))

    if (blogsSlice.putUnsubscribeBlog.fulfilled.match(promise)) {
      dispatch(
        snackbarSlice.actions.setSnackbar({
          open: true,
          color: 'success',
          msg: 'Subscription removed',
        })
      )
    } else {
      dispatch(
        snackbarSlice.actions.setSnackbar({
          open: true,
          color: 'error',
          msg: 'Removing subscription failed',
        })
      )
    }
    setIsInAction(false)
  }

  function getBtnBody() {
    if (!isInAction) return <DeleteIcon />
    return <SpinnerElement size={14} />
  }

  return (
    <Button
      aria-label="delete"
      onClick={() => {
        unsubcribeBlog()
      }}
    >
      {getBtnBody()}Unsubscribe
    </Button>
  )
}

export default UnsubscribeBlogBtn
