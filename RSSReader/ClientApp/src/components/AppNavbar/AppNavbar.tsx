import React from 'react'
import { Nav, Navbar } from 'react-bootstrap'
import { useHistory, useParams } from 'react-router-dom'
import { layoutSlice } from 'store/slices'
import UserNavbar from './UserNavbar'
import { useSelector } from 'react-redux'

export interface AppNavbarProps {}

const selectAuth = (state: { auth: any }) => state.auth

export const AppNavbar: React.FC<AppNavbarProps> = props => {
  const { push } = useHistory()
  const { loader } = useSelector(layoutSlice.stateSelector)
  const { groupId } = useParams<{ groupId: string }>()
  const renderNavar = () => {
    if (loader != layoutSlice.type.fullScreen) {
      return (
        <Navbar collapseOnSelect expand="lg" bg="dark" variant="dark">
          <Navbar.Brand href="#home">RSS Reader</Navbar.Brand>
          <Navbar.Toggle aria-controls="responsive-navbar-nav" />
          <Navbar.Collapse id="responsive-navbar-nav">
            <Nav className="mr-auto"></Nav>
            <UserNavbar curGroupId={parseInt(groupId)} />
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
