// @material-ui/core components
import { Box, Paper } from '@material-ui/core'
import { makeStyles } from '@material-ui/core/styles'
// @material-ui/icons
import Dashboard from '@material-ui/icons/Dashboard'
import List from '@material-ui/icons/List'
import Schedule from '@material-ui/icons/Schedule'
import appLookImg from 'components/Main/assets/img/applook.jpg'
import styles from 'components/Main/assets/jss/material-kit-react/views/componentsSections/pillsStyle.js'
import Button from 'components/Main/components/CustomButtons/Button.js'
// core components
import GridContainer from 'components/Main/components/Grid/GridContainer.js'
import GridItem from 'components/Main/components/Grid/GridItem.js'
import NavPills from 'components/Main/components/NavPills/NavPills.js'
import React from 'react'

const useStyles = makeStyles(styles)

export default function ParagraphOne() {
  const classes = useStyles()

  return (
    <div className={classes.sectionbig}>
      <div className={classes.containercenter}>
        <div id="navigation-pills">
          <GridContainer justify="center">
            <GridItem xs={12} sm={12} md={8}>
              <h1>Join us now</h1>
              <Button color="primary" size="lg">
                Sign up
              </Button>
            </GridItem>
          </GridContainer>
        </div>
      </div>
    </div>
  )
}
