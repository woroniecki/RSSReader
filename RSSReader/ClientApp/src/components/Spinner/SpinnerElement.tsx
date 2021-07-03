import React from 'react'
import { CircularProgress } from '@material-ui/core'

export interface SpinnerElementProps {
  size: number
}

export const SpinnerElement: React.FC<SpinnerElementProps> = props => {
  return <CircularProgress size={props.size} />
}

export default SpinnerElement
