import Login from 'components/Auth/Login'
import Register from 'components/Auth/Register'
import React from 'react'
import { Route, Switch } from 'react-router'
import {} from 'styled-components'
import MainPage from './Main/MainPage'

export interface HomeAppProps {}

export const HomeApp: React.FC<HomeAppProps> = props => {
  return (
    <>
      <MainPage />

      <Switch>
        <Route path="/login" component={Login} />
        <Route path="/register" component={Register} />
      </Switch>
    </>
  )
}

export default HomeApp
