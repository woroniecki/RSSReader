import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  TextField,
} from '@material-ui/core'
import React, { MouseEventHandler, useState } from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'

export interface DeleteGroupPromptProps {
  onMove: (event: React.MouseEvent<HTMLButtonElement>) => void
  onDelete: (event: React.MouseEvent<HTMLButtonElement>) => void
  onClose: (event: React.MouseEvent<HTMLButtonElement>) => void
}

export const DeleteGroupPrompt: React.FC<DeleteGroupPromptProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()

  const [show, setShow] = useState(false)

  const handleClose = () => setShow(false)
  const handleShow = () => setShow(true)

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
