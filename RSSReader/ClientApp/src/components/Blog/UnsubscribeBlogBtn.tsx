import React, { useState } from 'react'
import IconButton from '@material-ui/core/IconButton'
import DeleteIcon from '@material-ui/icons/Delete'
import { useAppDispatch } from 'store/store'
import { blogsSlice } from 'store/slices'

export interface UnsubscribeBlogBtnProps {
  id: number
}

export const UnsubscribeBlogBtn: React.FC<UnsubscribeBlogBtnProps> = props => {
  const dispatch = useAppDispatch()
  const [isInAction, setIsInAction] = useState(false)

  const unsubcribeBlog = async (id: number) => {
    if (isInAction) return
    setIsInAction(true)
    const promise = await dispatch(blogsSlice.putUnsubscribeBlog(id))

    if (blogsSlice.putUnsubscribeBlog.fulfilled.match(promise)) {
    } else {
    }
    setIsInAction(false)
  }

  return (
    <IconButton
      aria-label="delete"
      onClick={() => {
        unsubcribeBlog(props.id)
      }}
    >
      <DeleteIcon />
    </IconButton>
  )
}

export default UnsubscribeBlogBtn
