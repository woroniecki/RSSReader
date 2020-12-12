import React from 'react'
import ReactDOM from 'react-dom'
import './index.css'
import $ from 'jquery'
import App from './App'
import reportWebVitals from './reportWebVitals'
import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap/dist/js/bootstrap.bundle.js'
import { createBrowserHistory } from 'history'
import { BrowserRouter as Router, Switch, Route, Link } from 'react-router-dom'

import AppNavbar from './components/AppNavbar'
import AppHome from './components/AppHome'
import Register from './components/Register'
import Login from './components/Login'
import { ThemeProvider } from '@material-ui/core/styles'
import { createMuiTheme } from '@material-ui/core/styles'
import { Provider } from 'react-redux'
import { store } from 'store/store'
// A custom theme for this app
const theme = createMuiTheme()

const history = createBrowserHistory()

ReactDOM.render(
  <React.StrictMode>
    <Provider store={store}>
      <ThemeProvider theme={theme}>
        <Router history={history}>
          <AppNavbar />
          <Switch>
            <Route exact path="/" component={AppHome} />
            <Route path="/login" component={Login} />
            <Route path="/register" component={Register} />
            <Route>404</Route>
          </Switch>
        </Router>
      </ThemeProvider>
    </Provider>
  </React.StrictMode>,
  document.getElementById('root')
)

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals()
