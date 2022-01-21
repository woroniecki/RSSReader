import { Typography, TypographyProps } from '@material-ui/core'
import React from 'react'
import { useHistory } from 'react-router-dom'
import { useAppDispatch } from 'store/store'
import styled, { css } from 'styled-components'

export const Wrapper = styled.div<{ weigth?: number }>`
button {
}
  ${_ =>
    _.weigth &&
    css`
      weight: ${_.weigth};
    `}
  }
`
export interface AppTypographyProps extends TypographyProps {
  weight?: number
}

export const AppTypography: React.FC<AppTypographyProps> = ({ weight, ...props }) => {
  const dispatch = useAppDispatch()
  const { push } = useHistory()

  return (
    <div style={{wordBreak: "break-word"}}> 
      <Typography {...props} />
    </div>
  )
}

export default AppTypography
