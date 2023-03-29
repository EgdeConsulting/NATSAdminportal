// https://github.com/architanayak/react-table-demo/blob/main/src/Filter.js

import { useMemo, useState } from "react";
import { useAsyncDebounce } from "react-table";
import { Input, Select, FormControl, FormLabel } from "@chakra-ui/react";

export function GlobalFilter(props: {
  preGlobalFilteredRows: any;
  globalFilter: any;
  setGlobalFilter: any;
}) {
  const [value, setValue] = useState(props.globalFilter);

  const onChange = useAsyncDebounce((value) => {
    props.setGlobalFilter(value || undefined);
  }, 200);

  return (
    <FormControl p={0} m={0}>
      <FormLabel mt={-1}>
        Search Table:
        <Input
          mt={0}
          ml={4}
          value={value || ""}
          onChange={(e) => {
            setValue(e.target.value);
            onChange(e.target.value);
          }}
          placeholder="Enter value"
        />
      </FormLabel>
    </FormControl>
  );
}

// Component for Default Column Filter
export function DefaultFilterForColumn(props: {
  column: {
    filterValue: any;
    preFilteredRows: { length: any };
    setFilter: any;
  };
}) {
  return null;
}

// Component for Custom Select Filter
export function SelectColumnFilter(props: {
  column: {
    filterValue: any;
    setFilter: any;
    preFilteredRows: any;
    id: number;
  };
}) {
  // Use preFilteredRows to calculate the options
  const options = useMemo(() => {
    const options = new Set();
    props.column.preFilteredRows.forEach((row: any) => {
      options.add(row.values[props.column.id]);
    });
    return [...options.values()];
  }, [props.column.id, props.column.preFilteredRows]);

  // UI for Multi-Select box
  return (
    <Select
      value={props.column.filterValue}
      onChange={(e) => {
        props.column.setFilter(e.target.value || undefined);
      }}
      h={"35px"}
      mt={2}
    >
      <option value={""}>All</option>
      {options.map((option: any, i: number) => (
        <option key={i} value={option}>
          {option}
        </option>
      ))}
    </Select>
  );
}
