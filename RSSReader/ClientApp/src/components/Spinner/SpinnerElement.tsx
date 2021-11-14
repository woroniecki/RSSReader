import { CircularProgress } from '@material-ui/core'
import React from 'react'

export interface SpinnerElementProps {
  size: number
}

export const SpinnerElement: React.FC<SpinnerElementProps> = props => {
  return <CircularProgress size={props.size} />
}

export default SpinnerElement
