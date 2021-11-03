import { BottomNavigation, BottomNavigationAction } from '@material-ui/core'
import React, { useState } from 'react'
import PostAddIcon from '@material-ui/icons/PostAdd'
import DeleteIcon from '@material-ui/icons/Delete'
import AddIcon from '@material-ui/icons/Add'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { drawerWidth } from '../../../App'
import AddGroupFormPrompt from '../NavbarGroups/AddGroupFormPrompt'
import SubscribeFormPrompt from './SubscribeFormPrompt'

export interface ActionsBarProps {}

export const ActionsBar: React.FC<ActionsBarProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const [showGroupPrompt, setShowGroupPrompt] = useState(false)
  const [showSubscribePrompt, setShowSubscribePrompt] = useState(false)

  const renderAddGroupPrompt = () => {
    if (showGroupPrompt) {
      return <AddGroupFormPrompt onClose={() => setShowGroupPrompt(false)} />
    } else if (showSubscribePrompt) {
      return (
        <SubscribeFormPrompt onClose={() => setShowSubscribePrompt(false)} />
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
          console.log(newValue)
          switch (newValue) {
            case 0:
              setShowSubscribePrompt(true)
              break
            case 1:
              setShowGroupPrompt(true)
              break
            case 2:
              setShowGroupPrompt(true)
              break
          }
        }}
      >
        <BottomNavigationAction label="Subscribe" icon={<AddIcon />} />
        <BottomNavigationAction label="Add" icon={<PostAddIcon />} />
        <BottomNavigationAction label="Remove" icon={<DeleteIcon />} />
      </BottomNavigation>
      {renderAddGroupPrompt()}
    </>
  )
}

export default ActionsBar
