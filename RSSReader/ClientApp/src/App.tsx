import logo from './logo.svg'
import './App.css'
import React from 'react'
import {} from 'styled-components'
import AppHome from 'components/AppHome'
import AppNavbar from 'components/AppNavbar/AppNavbar'
import Login from 'components/Auth/Login'
import Register from 'components/Auth/Register'
import { Router, Switch, Route } from 'react-router'
import AppSpinner from './components/AppSpinner'
import { useSelector } from 'react-redux'
import { layoutSlice } from 'store/slices'
import useRefreshToken from 'components/Auth/useRefreshToken'
import SingleBlog from 'components/Blog/SingleBlog'
import SingleArticle from 'components/Article/SingleArticle'

function App() {
  const { loader } = useSelector(layoutSlice.stateSelector)
  useRefreshToken()
  return (
    <>
      <AppNavbar />
      <Switch>
        <Route path="/login" component={Login} />
        <Route path="/register" component={Register} />
        <Route path="/blog/:id" exact component={SingleBlog} />
        <Route path="/blog/:id/article/:articleid" component={SingleArticle} />
        <Route exact path={['/', '/:groupId']} component={AppHome} />
        <Route>404</Route>
      </Switch>
      {loader && <AppSpinner />}
    </>
  )
}

export default App
