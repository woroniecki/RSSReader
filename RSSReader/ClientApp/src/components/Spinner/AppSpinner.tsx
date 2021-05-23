import React from 'react'
import { Spinner } from 'react-bootstrap'
import { useHistory } from 'react-router-dom'

export interface AppSpinnerProps {}

export const AppSpinner: React.FC<AppSpinnerProps> = props => {
  return (
    <div className="spinner-wrapper">
      <Spinner animation="border" variant="danger" />
    </div>
  )
}

export default AppSpinner
