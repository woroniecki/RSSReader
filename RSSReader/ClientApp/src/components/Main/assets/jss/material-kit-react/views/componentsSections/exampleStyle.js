import { containerFluid } from 'components/Main/assets/jss/material-kit-react.js'

import imagesStyle from 'components/Main/assets/jss/material-kit-react/imagesStyles.js'

const exampleStyle = {
  section: {
    padding: '70px 0',
  },
  container: {
    ...containerFluid,
    textAlign: 'center !important',
  },
  ...imagesStyle,
  link: {
    textDecoration: 'none',
  },
}

export default exampleStyle
