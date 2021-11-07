import { BottomNavigation, BottomNavigationAction } from '@material-ui/core'
import React, { useState } from 'react'
import PostAddIcon from '@material-ui/icons/PostAdd'
import DeleteIcon from '@material-ui/icons/Delete'
import AddIcon from '@material-ui/icons/Add'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { drawerWidth } from '../../../App'
import AddGroupFormPrompt from './AddGroupFormPrompt'
import SubscribeFormPrompt from './SubscribeFormPrompt'
import { useParams } from 'react-router-dom'
import DeleteGroupPrompt from './DeleteGroupPrompt'

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
          disabled={isNaN(parseInt(groupId))}
          label="Remove"
          icon={<DeleteIcon />}
        />
      </BottomNavigation>
      {renderPrompt()}
    </>
  )
}

export default ActionsBar
