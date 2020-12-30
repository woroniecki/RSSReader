import { combineReducers } from '@reduxjs/toolkit'
import authReducer from './slices/authSlice'
import layoutSlice from './slices/layoutSlice'
import subscriptionsReducer from './slices/subscriptionsSlice'

export const rootReducer = combineReducers({
  authReducer,
  layoutSlice,
  subscriptionsReducer,
})

export type RootState = ReturnType<typeof rootReducer>
