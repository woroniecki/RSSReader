import React from 'react'
import { InputGroup, Button, FormControl } from 'react-bootstrap'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'

export interface AddSubProps {}

export const AddSub: React.FC<AddSubProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()

  return (
    <InputGroup className="mb-3">
      <InputGroup.Prepend>
        <Button variant="outline-primary">+</Button>
      </InputGroup.Prepend>
      <FormControl
        placeholder="https://exampleblog.com"
        aria-describedby="basic-addon1"
      />
    </InputGroup>
  )
}

export default AddSub
