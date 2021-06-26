import { Button } from '@material-ui/core'
import DeleteGroupPrompt from 'components/AppNavbar/NavbarGroups/DeleteGroupPrompt'
import React, { useState } from 'react'
import { useHistory, useParams } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { groupsSlice, subscriptionsSlice } from 'store/slices'
import DeleteIcon from '@material-ui/icons/Delete'
import { remove } from 'store/slices/groupsSlice'
import { useSelector } from 'react-redux'
import { Subscription } from 'api/api.types'

export interface RemoveGroupBtnProps {
  id: number
  curGroupId: number
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

      if (props.curGroupId == id) push('/')
    } else {
    }
  }

  const renderDeletePrompt = () => {
    if (showPrompt) {
      return (
        <DeleteGroupPrompt
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
      >
        <DeleteIcon />
      </Button>
      {renderDeletePrompt()}
    </React.Fragment>
  )
}

export default RemoveGroupBtn
