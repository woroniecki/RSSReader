// @material-ui/core components
import { makeStyles } from '@material-ui/core/styles'
// nodejs library that concatenates classes
import classNames from 'classnames'
import bgImg from 'components/Main/assets/img/landing7.jpg'
import styles from 'components/Main/assets/jss/material-kit-react/views/components.js'
import Button from 'components/Main/components/CustomButtons/Button.js'
import GridContainer from 'components/Main/components/Grid/GridContainer.js'
import GridItem from 'components/Main/components/Grid/GridItem.js'
// @material-ui/icons
// core components
import Header from 'components/Main/components/Header/Header.js'
// sections for this page
import HeaderLinks from 'components/Main/components/Header/HeaderLinks.js'
import Parallax from 'components/Main/components/Parallax/Parallax.js'
import React from 'react'
import { useHistory } from 'react-router-dom'
import ParagraphFooter from './ParagraphFooter.js'
// react components for routing our app without refresh
import ParagraphOne from './ParagraphOne.js'
import ParagraphThree from './ParagraphThree.js'
import ParagraphTwo from './ParagraphTwo.js'

const useStyles = makeStyles(styles)

export default function MainPage(props) {
  const classes = useStyles()
  const { push } = useHistory()
  const { ...rest } = props
  return (
    <div style={{ backgroundColor: '#000000' }}>
      <Header
        brand="RSS Box"
        rightLinks={<HeaderLinks />}
        fixed
        color="transparent"
        changeColorOnScroll={{
          height: 400,
          color: 'white',
        }}
        {...rest}
      />
      <Parallax image={bgImg}>
        <div className={classes.container}>
          <GridContainer>
            <GridItem>
              <div className={classes.brand}>
                <h1 className={classes.title}>A nice place to read.</h1>
                <h3 className={classes.subtitle}>
                  Follow your interests with RSS and keep everything in one
                  place.
                </h3>
                <Button
                  onClick={() => push('/register')}
                  color="primary"
                  size="lg"
                >
                  Sign up
                </Button>
              </div>
            </GridItem>
          </GridContainer>
        </div>
      </Parallax>

      <div className={classNames(classes.main, classes.mainRaised)}>
        <ParagraphOne />
        <ParagraphTwo />
        <ParagraphThree />
        <ParagraphFooter />
      </div>
    </div>
  )
}
