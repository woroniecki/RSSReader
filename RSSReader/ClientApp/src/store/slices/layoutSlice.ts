import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import { RootState } from 'store/rootReducer'

export enum type {
  none,
  partial,
  fullScreen,
}

interface LayoutState {
  loader?: type
}

const initialState: LayoutState = {
  loader: type.none,
}

const layoutSlice = createSlice({
  name: 'layout',
  initialState,
  reducers: {
    setLoader: (state, action: PayloadAction<type>) => {
      state.loader = action.payload
    },
  },
})

export const { actions } = layoutSlice

export default layoutSlice.reducer

export const stateSelector = (state: RootState): LayoutState =>
  state.layoutSlice
