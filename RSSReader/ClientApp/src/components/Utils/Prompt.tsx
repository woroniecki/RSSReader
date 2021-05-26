import React, { MouseEventHandler, useState } from 'react'
import { Button, Modal } from 'react-bootstrap'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'

export interface PromptProps {
  onMove: (event: React.MouseEvent<HTMLButtonElement>) => void
  onDelete: (event: React.MouseEvent<HTMLButtonElement>) => void
  onClose: (event: React.MouseEvent<HTMLButtonElement>) => void
}

export const Prompt: React.FC<PromptProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()

  const [show, setShow] = useState(false)

  const handleClose = () => setShow(false)
  const handleShow = () => setShow(true)

  return (
    <>
      <Modal show={true} onHide={props.onClose}>
        <Modal.Header closeButton>
          <Modal.Title>Modal heading</Modal.Title>
        </Modal.Header>
        <Modal.Body>What to do with blogs assigned to group? `</Modal.Body>
        <Modal.Footer>
          <Button variant="primary" onClick={props.onMove}>
            Move to all
          </Button>
          <Button variant="primary" onClick={props.onDelete}>
            Delete
          </Button>
          <Button variant="secondary" onClick={props.onClose}>
            Close
          </Button>
        </Modal.Footer>
      </Modal>
    </>
  )
}

export default Prompt
