import React from 'react'
import { Button, Nav, NavDropdown } from 'react-bootstrap'
import { Link, useHistory } from 'react-router-dom'
import { useSelector } from 'react-redux'
import { authSlice } from 'store/slices'
import { subscriptionsSlice } from 'store/slices'
import { useAppDispatch } from 'store/store'
import GroupsNavlist from './NavbarGroups/GroupsNavlist'

export interface UserNavbarProps {}

export const UserNavbar: React.FC<UserNavbarProps> = props => {
  const { push } = useHistory()
  const { userName } = useSelector(authSlice.stateSelector)
  const dispatch = useAppDispatch()

  let returnValue

  function OnLogout() {
    dispatch(authSlice.actions.logout())
    dispatch(subscriptionsSlice.actions.clear())
  }

  if (!userName) {
    returnValue = (
      <Nav>
        <Link to="/login" className="nav-link">
          Login
        </Link>
        <Link to="/register" className="nav-link">
          Register
        </Link>
      </Nav>
    )
  } else {
    returnValue = (
      <React.Fragment>
        <GroupsNavlist />
        <Nav>
          <NavDropdown title={userName} id="collasible-nav-dropdown" alignRight>
            <NavDropdown.Item href="">Panel</NavDropdown.Item>
            <NavDropdown.Divider />
            <Button className="dropdown-item" role="button" onClick={OnLogout}>
              Logout
            </Button>
          </NavDropdown>
        </Nav>
      </React.Fragment>
    )
  }

  return returnValue
}

export default UserNavbar
