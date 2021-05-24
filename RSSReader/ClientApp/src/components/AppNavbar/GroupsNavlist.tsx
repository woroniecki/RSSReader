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
import { LinkContainer } from 'react-router-bootstrap'

import { authSlice, groupsSlice, subscriptionsSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import AddGroupNavForm from './AddGroupNavForm'

export interface GroupsNavlistProps {}

export const GroupsNavlist: React.FC<GroupsNavlistProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()
  const { token } = useSelector(authSlice.stateSelector)
  const groupsList = useSelector(groupsSlice.selectAll)
  const subscriptionsList = useSelector(subscriptionsSlice.selectAll)

  const fetchList = async () => {
    const promise = await dispatch(groupsSlice.getList())

    if (groupsSlice.getList.fulfilled.match(promise)) {
    } else {
    }
  }

  const removeGroup = async (id: number) => {
    const promise = await dispatch(
      groupsSlice.remove({
        groupId: id,
        unsubscribeSubscriptions: false,
      })
    )

    if (groupsSlice.remove.fulfilled.match(promise)) {
    } else {
    }
  }

  React.useEffect(() => {
    if (token) {
      fetchList()
    }
  }, [token])

  const renderAllGroup = () => {
    return (
      <LinkContainer exact to={'/'}>
        <NavDropdown.Item>All</NavDropdown.Item>
      </LinkContainer>
    )
  }

  const renderNoneGroup = () => {
    if (subscriptionsList.filter(x => x.groupId == -1).length > 0) {
      return (
        <LinkContainer to={'/-1'}>
          <NavDropdown.Item>None</NavDropdown.Item>
        </LinkContainer>
      )
    }
  }

  const renderGroupsList = () =>
    groupsList
      .filter(el => el.id != -1)
      .map(el => (
        <LinkContainer to={'/' + el.id.toString()} key={el.id}>
          <NavDropdown.Item>
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
        </LinkContainer>
      ))

  return (
    <Nav>
      <NavDropdown title="Groups" id="collasible-nav-dropdown" alignRight>
        {renderAllGroup()}
        {renderNoneGroup()}
        {renderGroupsList()}
        <AddGroupNavForm />
      </NavDropdown>
    </Nav>
  )
}

export default GroupsNavlist
