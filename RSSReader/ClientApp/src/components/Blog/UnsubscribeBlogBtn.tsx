import React, { useState } from 'react'
import { Button } from 'react-bootstrap'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { subscriptionsSlice } from 'store/slices'
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
    const promise = await dispatch(subscriptionsSlice.putUnsubscribeBlog(id))

    if (subscriptionsSlice.putUnsubscribeBlog.fulfilled.match(promise)) {
    } else {
    }
    setIsInAction(false)
  }

  function getTrashBtnBody() {
    if (!isInAction) return <FontAwesomeIcon icon={faTrashAlt} />
    return <SpinnerElement variant="light" size={15} />
  }

  return (
    <Button
      onClick={() => {
        unsubcribeBlog(props.id)
      }}
      variant="primary"
    >
      {getTrashBtnBody()}
    </Button>
  )
}

export default UnsubscribeBlogBtn
