import { combineReducers } from '@reduxjs/toolkit'
import authReducer from './slices/authSlice'
import layoutSlice from './slices/layoutSlice'

export const rootReducer = combineReducers({ authReducer, layoutSlice })

export type RootState = ReturnType<typeof rootReducer>
