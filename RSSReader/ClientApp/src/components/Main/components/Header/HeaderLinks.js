/*eslint-disable*/
import List from '@material-ui/core/List'
import ListItem from '@material-ui/core/ListItem'
// @material-ui/core components
import { makeStyles } from '@material-ui/core/styles'
// @material-ui/icons
import { Create, Launch } from '@material-ui/icons'
import styles from 'components/Main/assets/jss/material-kit-react/components/headerLinksStyle.js'
import Button from 'components/Main/components/CustomButtons/Button.js'
import React from 'react'
import { useHistory } from 'react-router-dom'

const useStyles = makeStyles(styles)

export default function HeaderLinks(props) {
  const classes = useStyles()
  const { push } = useHistory()

  return (
    <List className={classes.list}>
      <ListItem className={classes.listItem}>
        <Button
          onClick={() => push(`/login`)}
          color="transparent"
          target="_blank"
          className={classes.navLink}
        >
          <Launch className={classes.icons} /> Login
        </Button>
      </ListItem>
      <ListItem className={classes.listItem}>
        <Button
          onClick={() => push(`/register`)}
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
