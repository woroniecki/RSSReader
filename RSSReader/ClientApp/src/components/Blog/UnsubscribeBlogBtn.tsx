import React, { useState } from 'react'
import IconButton from '@material-ui/core/IconButton'
import DeleteIcon from '@material-ui/icons/Delete'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { blogsSlice } from 'store/slices'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons'
import SpinnerElement from 'components/Spinner/SpinnerElement'

export interface UnsubscribeBlogBtnProps {
  id: number
}

export const UnsubscribeBlogBtn: React.FC<UnsubscribeBlogBtnProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
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

  function getTrashBtnBody() {
    if (!isInAction) return <FontAwesomeIcon icon={faTrashAlt} />
    return <SpinnerElement variant="light" size={15} />
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
