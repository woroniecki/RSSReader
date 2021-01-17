import logo from './logo.svg'
import './App.css'
import React from 'react'
import {} from 'styled-components'
import AppHome from 'components/AppHome'
import AppNavbar from 'components/AppNavbar'
import Login from 'components/Login'
import Register from 'components/Register'
import { Router, Switch, Route } from 'react-router'
import AppSpinner from './components/AppSpinner'
import { useSelector } from 'react-redux'
import { layoutSlice } from 'store/slices'
import useRefreshToken from 'components/useRefreshToken'
import SingleBlog from 'components/SingleBlog'

function App() {
  const { loader } = useSelector(layoutSlice.stateSelector)
  useRefreshToken()
  return (
    <>
      <AppNavbar />
      <Switch>
        <Route exact path="/" component={AppHome} />
        <Route path="/login" component={Login} />
        <Route path="/register" component={Register} />
        <Route path="/blog/:id" component={SingleBlog} />
        <Route>404</Route>
      </Switch>
      {loader && <AppSpinner />}
    </>
  )
}

export default App
