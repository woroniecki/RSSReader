import {
  container,
  title,
} from 'components/Main/assets/jss/material-kit-react.js'

import imagesStyle from 'components/Main/assets/jss/material-kit-react/imagesStyles.js'

const pillsStyle = {
  section: {
    padding: '70px 0',
  },
  sectiongrey: {
    background: '#CCCCCC',
    padding: '70px 0',
  },
  sectionbig: {
    padding: '100px 0 100px',
  },
  container,
  containercenter: {
    ...container,
    textAlign: 'center !important',
  },
  containerright: {
    ...container,
    textAlign: 'right !important',
  },
  title: {
    ...title,
    marginTop: '30px',
    minHeight: '32px',
    textDecoration: 'none',
  },
  ...imagesStyle,
}

export default pillsStyle
