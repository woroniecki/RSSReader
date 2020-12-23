import { combineReducers } from '@reduxjs/toolkit'
import authReducer from './slices/authSlice'

export const rootReducer = combineReducers({ authReducer })

export type RootState = ReturnType<typeof rootReducer>
