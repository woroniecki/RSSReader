/*eslint-disable*/
import React from 'react'
import DeleteIcon from '@material-ui/icons/Delete'
import IconButton from '@material-ui/core/IconButton'
// react components for routing our app without refresh
import { Link } from 'react-router-dom'

// @material-ui/core components
import { makeStyles } from '@material-ui/core/styles'
import List from '@material-ui/core/List'
import ListItem from '@material-ui/core/ListItem'
import Tooltip from '@material-ui/core/Tooltip'

// @material-ui/icons
import { Apps, Create, Launch } from '@material-ui/icons'

// core components
import CustomDropdown from 'components/Main/components/CustomDropdown/CustomDropdown.js'
import Button from 'components/Main/components/CustomButtons/Button.js'

import styles from 'components/Main/assets/jss/material-kit-react/components/headerLinksStyle.js'

const useStyles = makeStyles(styles)

export default function HeaderLinks(props) {
  const classes = useStyles()
  return (
    <List className={classes.list}>
      <ListItem className={classes.listItem}>
        <Button
          href="https://www.creative-tim.com/product/material-kit-react?ref=mkr-navbar"
          color="transparent"
          target="_blank"
          className={classes.navLink}
        >
          <Launch className={classes.icons} /> Login
        </Button>
      </ListItem>
      <ListItem className={classes.listItem}>
        <Button
          href="https://www.creative-tim.com/product/material-kit-react?ref=mkr-navbar"
          color="transparent"
          target="_blank"
          className={classes.navLink}
        >
          <Create className={classes.icons} /> Register
        </Button>
      </ListItem>
    </List>
  )
}
