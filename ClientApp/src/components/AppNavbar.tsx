import React from 'react'
import { Nav, Navbar, NavDropdown } from 'react-bootstrap'
import { Link, useHistory } from 'react-router-dom'
import UserNavbar from './AppNavbar/UserNavbar'

export interface AppNavbarProps {}

const selectAuth = (state: { auth: any }) => state.auth

export const AppNavbar: React.FC<AppNavbarProps> = props => {
  const { push } = useHistory()

  return (
    <Navbar collapseOnSelect expand="lg" bg="dark" variant="dark">
      <Navbar.Brand href="#home">RSS Reader</Navbar.Brand>
      <Navbar.Toggle aria-controls="responsive-navbar-nav" />
      <Navbar.Collapse id="responsive-navbar-nav">
        <Nav className="mr-auto"></Nav>
        <UserNavbar />
      </Navbar.Collapse>
    </Navbar>
  )
}

export default AppNavbar
