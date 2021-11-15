import Login from 'components/Auth/Login'
import Register from 'components/Auth/Register'
import React from 'react'
import { Route, Switch } from 'react-router'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'

export interface HomeAppProps {}

export const HomeApp: React.FC<HomeAppProps> = props => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()

  return (
    <Switch>
      <Route path="/login" component={Login} />
      <Route path="/register" component={Register} />
      <Route>404</Route>
    </Switch>
  )
}

export default HomeApp
