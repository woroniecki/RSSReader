// @material-ui/core components
import { Box, Paper } from '@material-ui/core'
import { makeStyles } from '@material-ui/core/styles'
// @material-ui/icons
import Dashboard from '@material-ui/icons/Dashboard'
import List from '@material-ui/icons/List'
import Schedule from '@material-ui/icons/Schedule'
import computerImg from 'components/Main/assets/img/computer.png'
import styles from 'components/Main/assets/jss/material-kit-react/views/componentsSections/pillsStyle.js'
// core components
import GridContainer from 'components/Main/components/Grid/GridContainer.js'
import GridItem from 'components/Main/components/Grid/GridItem.js'
import NavPills from 'components/Main/components/NavPills/NavPills.js'
import React from 'react'

const useStyles = makeStyles(styles)

export default function ParagraphOne() {
  const classes = useStyles()

  return (
    <div className={classes.sectiongrey}>
      <div className={classes.containerright}>
        <div id="navigation-pills">
          <GridContainer>
            <GridItem xs={11} sm={11} md={8} lg={12}>
              <Box
                style={{
                  width: 75,
                  height: 15,
                  backgroundColor: '#9c27b0',
                  marginLeft: 'auto',
                  marginRight: 0,
                }}
              />
              <div>
                <h1>Organize, Manage & Share</h1>
                <h3>
                  Stay organized, share your faves, and find that really great
                  article you read last month.
                </h3>
              </div>
            </GridItem>
          </GridContainer>
        </div>
      </div>
      <div className={classes.container}>
        <div id="navigation-pills">
          <GridContainer>
            <GridItem xs={11} sm={11} md={8} lg={4}>
              <h2>Full-Text</h2>
              <h4>
                Feedbin can help get full-text of an article for feeds that only
                offer partial-content. This way you can keep reading without
                leaving Feedbin.
              </h4>
              <h2>Updated Articles</h2>
              <h4>
                Articles are updated whenever the original changes so you dont
                miss any important changes. You can even see the differences to
                know what changed.
              </h4>
            </GridItem>
            <GridItem xs={11} sm={11} md={8} lg={8}>
              <img
                src={computerImg}
                alt="..."
                className={classes.imgRounded + ' ' + classes.imgFluid}
              />
            </GridItem>
          </GridContainer>
        </div>
      </div>
    </div>
  )
}
