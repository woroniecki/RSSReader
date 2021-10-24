import React, { useState } from 'react'
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
} from '@material-ui/core'
import SpinnerElement from 'components/Spinner/SpinnerElement'

export interface DeleteGroupPromptProps {
  onMove: (event: React.MouseEvent<HTMLButtonElement>) => void
  onDelete: (event: React.MouseEvent<HTMLButtonElement>) => void
  onClose: (event: React.MouseEvent<HTMLButtonElement>) => void
  enableSpinner: boolean
}

export const DeleteGroupPrompt: React.FC<DeleteGroupPromptProps> = props => {
  function renderProcessing() {
    if (!props.enableSpinner) return
    return <SpinnerElement size={24} />
  }

  return (
    <>
      <Dialog
        open={true}
        onClose={props.onClose}
        aria-labelledby="form-dialog-title"
      >
        <DialogTitle id="form-dialog-title">Delete group</DialogTitle>
        <DialogContent>
          <DialogContentText>
            What to do with blogs assigned to group?
          </DialogContentText>
          <div
            style={{
              display: 'flex',
              justifyContent: 'center',
              alignItems: 'center',
            }}
          >
            {renderProcessing()}
          </div>
        </DialogContent>
        <DialogActions>
          <Button onClick={props.onMove}>Move to all</Button>
          <Button onClick={props.onDelete}>Delete</Button>
          <Button onClick={props.onClose}>Close</Button>
        </DialogActions>
      </Dialog>
    </>
  )
}

export default DeleteGroupPrompt
