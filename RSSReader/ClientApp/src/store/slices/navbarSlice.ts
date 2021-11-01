import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import { RootState } from 'store/rootReducer'

interface NavbarState {
  navOpen?: boolean
}

const initialState: NavbarState = {
  navOpen: false,
}

const navbarSlice = createSlice({
  name: 'navbar',
  initialState,
  reducers: {
    setOpen: (state, action: PayloadAction<boolean>) => {
      state.navOpen = action.payload
    },
  },
})

export const { actions } = navbarSlice

export default navbarSlice.reducer

export const stateSelector = (state: RootState): NavbarState =>
  state.navbarSlice
