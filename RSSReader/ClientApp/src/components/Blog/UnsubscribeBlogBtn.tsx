import React, { useState } from 'react'
import IconButton from '@material-ui/core/IconButton'
import DeleteIcon from '@material-ui/icons/Delete'
import { useAppDispatch } from 'store/store'
import { blogsSlice } from 'store/slices'
import { Blog } from 'api/api.types'

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
    } else {
    }
    setIsInAction(false)
  }

  return (
    <IconButton
      aria-label="delete"
      onClick={() => {
        unsubcribeBlog()
      }}
    >
      <DeleteIcon />
    </IconButton>
  )
}

export default UnsubscribeBlogBtn
