import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import Prompt from 'components/Utils/Prompt'
import React, { useState } from 'react'
import { Button } from 'react-bootstrap'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { groupsSlice, subscriptionsSlice } from 'store/slices'
import { faTrashAlt } from '@fortawesome/free-solid-svg-icons'
import { remove } from 'store/slices/groupsSlice'
import { useSelector } from 'react-redux'
import { Subscription } from 'api/api.types'

export interface RemoveGroupBtnProps {
  id: number
}

export const RemoveGroupBtn: React.FC<RemoveGroupBtnProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const subscriptionsList = useSelector(subscriptionsSlice.selectAll)
  const [showPrompt, setShowPrompt] = useState(false)

  const getSubs = () => {
    const subs: Subscription[] = []

    subscriptionsList
      .filter(x => x.groupId == props.id)
      .map(el => subs.push(el))

    return subs
  }

  const removeGroup = async (id: number, moveSubsToAll: boolean) => {
    const subs_to_change = getSubs()

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
          dispatch(subscriptionsSlice.actions.resetGroup(sub.id))
        }
      } else {
        //remove all subs with unknown group
        for (const sub of subs_to_change) {
          dispatch(subscriptionsSlice.actions.remove(sub.id))
        }
      }
      push('/')
    } else {
    }
  }

  const renderDeletePrompt = () => {
    if (showPrompt) {
      return (
        <Prompt
          onMove={() => removeGroup(props.id, true)}
          onDelete={() => removeGroup(props.id, false)}
          onClose={() => setShowPrompt(false)}
        />
      )
    }
  }

  return (
    <React.Fragment>
      <Button
        onClick={() => {
          setShowPrompt(true)
        }}
        variant="primary"
      >
        <FontAwesomeIcon icon={faTrashAlt} />
      </Button>
      {renderDeletePrompt()}
    </React.Fragment>
  )
}

export default RemoveGroupBtn
