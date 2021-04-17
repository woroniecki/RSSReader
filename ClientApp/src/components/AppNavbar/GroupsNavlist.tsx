import React from 'react'
import {
  Button,
  Form,
  FormControl,
  InputGroup,
  Nav,
  NavDropdown,
} from 'react-bootstrap'
import { Link, useHistory } from 'react-router-dom'
import { useSelector } from 'react-redux'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faTrashAlt, faPlus } from '@fortawesome/free-solid-svg-icons'

import { authSlice, groupsSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import AddGroupNavForm from './AddGroupNavForm'

export interface GroupsNavlistProps {}

export const GroupsNavlist: React.FC<GroupsNavlistProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { token } = useSelector(authSlice.stateSelector)
  const groupsList = useSelector(groupsSlice.selectAll)

  const fetchList = async () => {
    const promise = await dispatch(groupsSlice.getList())

    if (groupsSlice.getList.fulfilled.match(promise)) {
    } else {
    }
  }

  const removeGroup = async (id: number) => {
    const promise = await dispatch(groupsSlice.remove(id))

    if (groupsSlice.remove.fulfilled.match(promise)) {
    } else {
    }
  }

  React.useEffect(() => {
    if (token) {
      fetchList()
    }
  }, [token])

  const renderGroupsList = () =>
    groupsList.map(el => (
      <NavDropdown.Item key={el.id}>
        {el.name}
        <Button
          onClick={() => {
            removeGroup(el.id)
          }}
          variant="primary"
        >
          <FontAwesomeIcon icon={faTrashAlt} />
        </Button>
      </NavDropdown.Item>
    ))

  return (
    <Nav>
      <NavDropdown title="Groups" id="collasible-nav-dropdown" alignRight>
        <NavDropdown.Item href="">All</NavDropdown.Item>
        <NavDropdown.Divider />
        {renderGroupsList()}
        <NavDropdown.Divider />
        <AddGroupNavForm />
      </NavDropdown>
    </Nav>
  )
}

export default GroupsNavlist
