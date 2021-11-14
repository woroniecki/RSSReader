import { BottomNavigation, BottomNavigationAction } from '@material-ui/core'
import AddIcon from '@material-ui/icons/Add'
import DeleteIcon from '@material-ui/icons/Delete'
import PostAddIcon from '@material-ui/icons/PostAdd'
import React, { useState } from 'react'
import { useHistory, useParams } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { drawerWidth } from '../../UserApp'
import AddGroupFormPrompt from './AddGroupFormPrompt'
import DeleteGroupPrompt from './DeleteGroupPrompt'
import SubscribeFormPrompt from './SubscribeFormPrompt'

export interface ActionsBarProps {}

export const ActionsBar: React.FC<ActionsBarProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { groupId } = useParams<{ groupId: string }>()
  const [showSubscribePrompt, setShowSubscribePrompt] = useState(false)
  const [showGroupPrompt, setShowGroupPrompt] = useState(false)
  const [showDeleteGroupPrompt, setShowDeleteGroupPrompt] = useState(false)

  const renderPrompt = () => {
    if (showGroupPrompt) {
      return <AddGroupFormPrompt onClose={() => setShowGroupPrompt(false)} />
    } else if (showSubscribePrompt) {
      return (
        <SubscribeFormPrompt onClose={() => setShowSubscribePrompt(false)} />
      )
    } else if (showDeleteGroupPrompt) {
      return (
        <DeleteGroupPrompt
          groupId={parseInt(groupId)}
          onClose={() => setShowDeleteGroupPrompt(false)}
        />
      )
    }
  }

  const IsDeleteGroupDisabled = () => {
    const value = parseInt(groupId)

    if (isNaN(value) || value < 0) return true

    return false
  }

  return (
    <>
      <BottomNavigation
        showLabels
        style={{ position: 'fixed', bottom: 0, width: drawerWidth - 1 }}
        value={''}
        onChange={(event, newValue) => {
          switch (newValue) {
            case 0:
              setShowSubscribePrompt(true)
              break
            case 1:
              setShowGroupPrompt(true)
              break
            case 2:
              setShowDeleteGroupPrompt(true)
              break
          }
        }}
      >
        <BottomNavigationAction label="Subscribe" icon={<AddIcon />} />
        <BottomNavigationAction label="Add" icon={<PostAddIcon />} />
        <BottomNavigationAction
          disabled={IsDeleteGroupDisabled()}
          label="Remove"
          icon={<DeleteIcon />}
        />
      </BottomNavigation>
      {renderPrompt()}
    </>
  )
}

export default ActionsBar
