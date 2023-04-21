import { Dispatch, SetStateAction, useMemo, useState } from "react";
import { Column, Row, useAsyncDebounce } from "react-table";
import {
  Input,
  Select,
  FormControl,
  FormLabel,
  TableOptions,
} from "@chakra-ui/react";

/**
 * The following code is based on this source:
 * https://github.com/architanayak/react-table-demo/blob/main/src/Filter.js
 */
export function GlobalFilter(props: {
  preGlobalFilteredRows: Row<{}>[];
  globalFilter: string;
  setGlobalFilter: Dispatch<SetStateAction<TableOptions>>;
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
    filterValue: string;
    preFilteredRows: { length: number };
    setFilter: Dispatch<SetStateAction<Column>>;
  };
}) {
  return null;
}

// Component for Custom Select Filter
export function SelectColumnFilter(props: {
  column: {
    filterValue: string;
    setFilter: Dispatch<SetStateAction<string | undefined>>;
    preFilteredRows: Row[];
    id: number;
  };
}) {
  // Use preFilteredRows to calculate the options
  const options = useMemo(() => {
    const options = new Set();
    props.column.preFilteredRows.forEach((row: Row) => {
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
      aria-label={"ColumnFilter"}
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
