import { Button, Grid, IconButton, Paper, Typography } from '@material-ui/core'
import React, { useState } from 'react'
import PostAddIcon from '@material-ui/icons/PostAdd'
import DeleteIcon from '@material-ui/icons/Delete'
import AddIcon from '@material-ui/icons/Add'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import NavbarAction from './NavbarAction'
import AddGroupFormPrompt from '../NavbarGroups/AddGroupFormPrompt'

export interface ActionsBarProps {}

export const ActionsBar: React.FC<ActionsBarProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const [showGroupPrompt, setGroupShowPrompt] = useState(false)

  const renderAddGroupPrompt = () => {
    if (showGroupPrompt) {
      return <AddGroupFormPrompt onClose={() => setGroupShowPrompt(false)} />
    }
  }

  return (
    <>
      <Paper style={{ position: 'fixed', bottom: 0, width: 290 }}>
        <NavbarAction label="Subscribe">
          <AddIcon />
        </NavbarAction>
        <NavbarAction label="Add" onClick={() => setGroupShowPrompt(true)}>
          <PostAddIcon />
        </NavbarAction>
        <NavbarAction label="Remove">
          <DeleteIcon />
        </NavbarAction>
      </Paper>
      {renderAddGroupPrompt()}
    </>
  )
}

export default ActionsBar
