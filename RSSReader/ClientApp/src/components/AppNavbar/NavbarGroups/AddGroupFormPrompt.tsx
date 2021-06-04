import React from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import { Button, Modal, Form, FormControl, InputGroup } from 'react-bootstrap'
import { useFormik } from 'formik'
import { groupsSlice } from 'store/slices'
import { applyValidationErrors } from 'utils/utils'

export interface AddGroupFormPromptProps {
  onClose: (event: React.MouseEvent<HTMLButtonElement>) => void
}

export const AddGroupFormPrompt: React.FC<AddGroupFormPromptProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()

  const formik = useFormik({
    initialValues: {
      global: '',
      name: '',
    },
    onSubmit: async values => {
      const promise = await dispatch(
        groupsSlice.postAdd({
          name: values.name,
        })
      )

      if (groupsSlice.postAdd.fulfilled.match(promise)) {
        props.onClose(this)
      } else {
        applyValidationErrors(formik, promise.error)
      }
    },
  })

  return (
    <>
      <Modal show={true} onHide={props.onClose}>
        <Modal.Header closeButton>
          <Modal.Title>Add group</Modal.Title>
        </Modal.Header>
        <Form id="AddSub" onSubmit={formik.handleSubmit}>
          <Modal.Body>
            <InputGroup className="mb-3">
              <FormControl
                type="text"
                aria-describedby="basic-addon1"
                placeholder="Group name"
                id="url"
                name="url"
                onChange={event => {
                  formik.values.name = event.target.value
                  formik.handleChange
                }}
                onBlur={formik.handleBlur}
                isInvalid={
                  !!formik.touched.name &&
                  (!!formik.errors.name || !!formik.errors.global)
                }
                required
              />
              <Form.Control.Feedback type="invalid">
                {formik.errors.global}
                {formik.errors.name}
              </Form.Control.Feedback>
            </InputGroup>
          </Modal.Body>
          <Modal.Footer>
            <Button
              //disabled={isLoading}
              //onClick={!isLoading ? handleClick : null}
              variant="outline-primary"
              type="submit"
            >
              Add
            </Button>

            <Button variant="secondary" onClick={props.onClose}>
              Close
            </Button>
          </Modal.Footer>
        </Form>
      </Modal>
    </>
  )
}

export default AddGroupFormPrompt
