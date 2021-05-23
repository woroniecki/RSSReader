import React from 'react'
import { Nav, Navbar, NavDropdown } from 'react-bootstrap'
import { Link, useHistory } from 'react-router-dom'
import { layoutSlice } from 'store/slices'
import UserNavbar from './UserNavbar'
import { useSelector } from 'react-redux'
import { ReactComponent } from '*.svg'

export interface AppNavbarProps {}

const selectAuth = (state: { auth: any }) => state.auth

export const AppNavbar: React.FC<AppNavbarProps> = props => {
  const { push } = useHistory()
  const { loader } = useSelector(layoutSlice.stateSelector)

  const renderNavar = () => {
    if (loader != layoutSlice.type.fullScreen) {
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
    } else {
      return <React.Fragment></React.Fragment>
    }
  }

  return renderNavar()
}

export default AppNavbar
