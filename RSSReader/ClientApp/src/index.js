import React from 'react'
import ReactDOM from 'react-dom'
import './index.css'
import $ from 'jquery'
import App from './App'
import reportWebVitals from './reportWebVitals'
import { createBrowserHistory } from 'history'
import { BrowserRouter as Router, Switch, Route, Link } from 'react-router-dom'
import { store } from 'store/store'
import { Provider } from 'react-redux'
import './api/axios'
import { ThemeProvider as StyledProvider } from 'styled-components'
import { ThemeProvider as MuiProvider, StylesProvider } from '@material-ui/core'
import { theme } from 'components/theme/theme'
// A custom theme for this app

const history = createBrowserHistory()

ReactDOM.render(
  <React.StrictMode>
    <Provider store={store}>
      <Router history={history}>
        <StyledProvider theme={theme}>
          <StylesProvider injectFirst>
            <MuiProvider theme={theme}>
              <App />
            </MuiProvider>
          </StylesProvider>
        </StyledProvider>{' '}
      </Router>
    </Provider>
  </React.StrictMode>,
  document.getElementById('root')
)

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals()
