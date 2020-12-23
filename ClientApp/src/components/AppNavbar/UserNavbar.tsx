import React from 'react'
import { Nav, NavDropdown } from 'react-bootstrap'
import { Link, useHistory } from 'react-router-dom'
import { authSlice } from 'store/slices'
import { useSelector } from 'react-redux'

export interface UserNavbarProps {}

export const UserNavbar: React.FC<UserNavbarProps> = props => {
  const { push } = useHistory()
  const { userName } = useSelector(authSlice.stateSelector)

  let returnValue

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
      <Nav>
        <Link to="/register" className="nav-link">
          {userName}
        </Link>
        <NavDropdown title={userName} id="collasible-nav-dropdown" alignRight>
          <NavDropdown.Item href="">Panel</NavDropdown.Item>
          <NavDropdown.Divider />
          <Link to="/register" className="dropdown-item" role="button">
            Logout
          </Link>
        </NavDropdown>
      </Nav>
    )
  }

  return returnValue
}

export default UserNavbar
