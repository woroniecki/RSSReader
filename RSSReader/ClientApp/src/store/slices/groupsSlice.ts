import {
  createAsyncThunk,
  createEntityAdapter,
  createSlice,
  PayloadAction,
} from '@reduxjs/toolkit'
import * as blogApi from '../../api/blogApi'
import { AddGroupRequest, Group } from '../../api/api.types'
import { RootState } from 'store/rootReducer'
import { NumberSchema } from 'yup'
const GROUPS = 'groups'

export const groupsAdapter = createEntityAdapter<Group>({
  selectId: x => x.id,
})

export const getList = createAsyncThunk<
  // Return type of the payload creator
  Group[],
  // First argument to the payload creator
  void,
  {
    rejectValue: string
  }
>(`${GROUPS}/list`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.getGroupsList()
    return res
  } catch (err) {
    return rejectWithValue(err.response.data)
  }
})

export const postAdd = createAsyncThunk<
  // Return type of the payload creator
  Group,
  // First argument to the payload creator
  AddGroupRequest,
  {
    rejectValue: string
  }
>(`${GROUPS}/add`, async (params, { rejectWithValue }) => {
  try {
    const res = await blogApi.postAddGroup(params)
    return res
  } catch (err) {
    throw err.data
  }
})

export const remove = createAsyncThunk<
  // Return type of the payload creator
  number,
  // First argument to the payload creator
  number,
  {
    rejectValue: string
  }
>(`${GROUPS}/remove`, async (id, { rejectWithValue }) => {
  try {
    const res = await blogApi.removeGroup(id)
    return id
  } catch (err) {
    throw err.data
  }
})

const groupsSlice = createSlice({
  name: 'groups',
  initialState: groupsAdapter.getInitialState(),
  reducers: {},
  extraReducers: builder => {
    builder
      .addCase(getList.fulfilled, (state, { payload }) => {
        groupsAdapter.setAll(state, payload)
      })
      .addCase(postAdd.fulfilled, (state, { payload }) => {
        groupsAdapter.addOne(state, payload)
      })
      .addCase(remove.fulfilled, (state, { payload }) => {
        groupsAdapter.removeOne(state, payload)
      })
  },
})

export const { actions } = groupsSlice

export default groupsSlice.reducer

export const stateSelector = (state: RootState) => state.groupsReducer

export const {
  selectAll,
  selectById,
  selectIds,
} = groupsAdapter.getSelectors<RootState>(state => state.groupsReducer)
